using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PX.Data;
using PX.Objects.CS;
using PX.Objects.IN;
using PX.Objects.EP;
using PX.Objects.GL;
using PX.Objects.CT;
using System.Collections;
using PX.Objects.CM;
using PX.Objects.CR;
using System.Diagnostics;
using PX.Objects.AR;
using PX.Objects.SO;
using PX.Objects.PM;
using static PX.Objects.PM.Messages;




namespace Kedge.DAC
{
    public class Utils
    {
        public static Segment GetSegment(PXGraph graph, int? dimensionId)
        {
            Segment seg = PXSelect<Segment,
                Where<Segment.dimensionID, Equal<Required<Segment.dimensionID>>>>.Select(graph, dimensionId);

            return seg;
        }

        public static bool RaiseNullCheck<Field>(PXCache cache  , object row, object newValue,String feildName) where Field : IBqlField{
            if (newValue == null)
            {
                cache.RaiseExceptionHandling<Field>(row, newValue,new PXSetPropertyException(feildName + " can't be null."));
                return false;
            }
            else {
                return true;
            }
        }
 
    }

    [PXDBInt()]
    [PXUIField(DisplayName = "Project", Visibility = PXUIVisibility.Visible, FieldClass = ProjectAttribute.DimensionName)]
    [PXRestrictor(typeof(Where<PMProject.isCompleted, Equal<False>>), PX.Objects.PM.Messages.CompleteContract, typeof(PMProject.contractCD))]
    [PXRestrictor(typeof(Where<PMProject.baseType, NotEqual<PX.Objects.CT.CTPRType.projectTemplate>,
        And<PMProject.baseType, NotEqual<PX.Objects.CT.CTPRType.contractTemplate>>>), PX.Objects.PM.Messages.TemplateContract, typeof(PMProject.contractCD))]
    public class ProjectBaseExtAttribute : AcctSubAttribute, IPXFieldVerifyingSubscriber
    {
        protected Type customerField;

        public ProjectBaseExtAttribute() : this(null)
        {

        }

        public ProjectBaseExtAttribute(Type customerField)
        {
            this.customerField = customerField;

            PXDimensionSelectorAttribute select;

            if (customerField == null)
            {
                select = new PXDimensionSelectorAttribute(ProjectAttribute.DimensionName,
                        typeof(Search2<PMProject.contractID,
                        LeftJoin<Customer, On<Customer.bAccountID, Equal<PMProject.customerID>>>,
                        Where<PMProject.baseType, Equal<CTPRType.project>,And<PMProject.defaultBranchID,Equal<Current<AccessInfo.branchID>>,
                        And<PMProject.status, Equal<ProjectStatus.active>,
                        And2<Match<Current<AccessInfo.userName>>, Or<PMProject.nonProject, Equal<True>>>>>>>)
                        , typeof(PMProject.contractCD), typeof(PMProject.contractCD), typeof(PMProject.description),
                        typeof(PMProject.status), typeof(PMProject.customerID), typeof(Customer.acctName), typeof(PMProject.curyID));
            }
            else
            {
                List<Type> command = new List<Type>();

                command.AddRange(new[] {
                typeof(Search2<,,>),
                    typeof(PMProject.contractID),
                    typeof(LeftJoin<,>),
                            typeof(Customer),
                            typeof(On<,>),
                            typeof(Customer.bAccountID),
                            typeof(Equal<>),
                            typeof(PMProject.customerID),
                        });

                command.AddRange(
                    new[]
                    {
                        typeof(Where<,,>),
                        typeof(PMProject.baseType),
                        typeof(Equal<>),
                        typeof(PX.Objects.CT.CTPRType.project),
                        typeof (And2<,>),
                        typeof (Where2<,>),
                        typeof (Where<,,>),
                        typeof (PMProject.customerID),
                        typeof (Equal<>),
                        typeof (Current<>),
                        customerField,
                        typeof (And<,,>),
                        typeof (PMProject.restrictProjectSelect),
                        typeof (Equal<>),
                        typeof (PMRestrictOption.customerProjects),
                        typeof (Or<,>),
                        typeof (PMProject.restrictProjectSelect),
                        typeof (Equal<>),
                        typeof (PMRestrictOption.allProjects),
                        typeof (Or<,>),
                        typeof (Current<>),
                        customerField,
                        typeof (IsNull),
                        typeof (And2<,>),
                        typeof (Match<Current<AccessInfo.userName>>),
                        typeof (Or<,>),
                        typeof (PMProject.nonProject),
                        typeof (Equal<>),
                        typeof (True)
                    });

                select = new PXDimensionSelectorAttribute(ProjectAttribute.DimensionName,
                BqlCommand.Compose(command.ToArray())
                , typeof(PMProject.contractCD), typeof(PMProject.contractCD), typeof(PMProject.description),
                typeof(PMProject.status), typeof(PMProject.customerID), typeof(Customer.acctName), typeof(PMProject.curyID));
            }

            select.DescriptionField = typeof(PMProject.description);
            select.ValidComboRequired = true;
            select.CacheGlobal = true;

            _Attributes.Add(select);
            _SelAttrIndex = _Attributes.Count - 1;

            Filterable = true;
        }

        public virtual void FieldVerifying(PXCache sender, PXFieldVerifyingEventArgs e)
        {

            if (customerField == null)
                return;

            PMProject project = PXSelect<PMProject>.Search<PMProject.contractID>(sender.Graph, e.NewValue);
            if (project != null && project.NonProject != true)
            {
                int? customerID = (int?)sender.GetValue(e.Row, customerField.Name);

                if (customerID != project.CustomerID)
                {
                    sender.RaiseExceptionHandling(FieldName, e.Row, e.NewValue,
                        new PXSetPropertyException(Warnings.ProjectCustomerDontMatchTheDocument, PXErrorLevel.Warning));
                }
            }
        }
    }
}
