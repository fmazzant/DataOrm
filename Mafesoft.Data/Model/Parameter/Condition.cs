/// Mafesoft.Data
/// <summary>An abstract accessing to database</summary>
///
///
///                                                                    o o
///                                                                  o     o
///                                                                 _   O  _
///  Copyright(C) 2006                                                \/)\/
///  Federico Mazzanti                                               /\/|
///                                                                     |
///                                                                     \
///  All rights reserved.

using Mafesoft.Data.Core.Parameter.Util;
using System;
using System.Collections.Generic;

namespace Mafesoft.Data.Core.Parameter
{
    /// <summary>
    /// Condition Type
    /// </summary>
    public enum ConditionType
    {
        None,
        And,
        Or
    }

    /// <summary>
    /// Condition Child Type
    /// </summary>
    internal enum ConditionChildType
    {
        None,
        Conditions,
        Parameters
    }

    /// <summary>
    /// Represents a Condition
    /// </summary>
    public class Condition
    {
        private List<Condition> childConditions = new List<Condition>();
        private List<RecordParameter> childParameters = new List<RecordParameter>();
        private Condition parent = null;

        protected Condition()
        {
            TypeOfCondition = ConditionType.None;
            TypeOfChildConditions = ConditionChildType.None;
            IsEmpty = false;
        }

        protected Condition(Condition pParent)
            : this()
        {
            parent = pParent;
        }

        /// <summary>
        /// Represents a empty condition.
        /// </summary>
        public static Condition Empty
        {
            get
            {
                Condition empty = new Condition();
                empty.IsEmpty = true;
                return empty;
            }
        }

        public Condition Parent { get { return parent; } }

        public ConditionType TypeOfCondition { get; set; }

        internal Boolean IsEmpty { get; private set; }

        internal ConditionChildType TypeOfChildConditions { get; set; }

        /// <summary>
        /// Represents an instance of Condition with internal conditions in AND operation
        /// </summary>
        /// <param name="pC1">First condition</param>
        /// <param name="pC2">Second condition</param>
        /// <param name="pConditions">List of conditions</param>
        /// <returns>Condition instance</returns>
        public static Condition AND(Condition pC1, params Condition[] pConditions)
        {
            Condition condition = new Condition();
            condition.TypeOfCondition = ConditionType.And;
            condition.TypeOfChildConditions = ConditionChildType.Conditions;

            pC1.SetParent(condition);
            condition.childConditions.Add(pC1);
            foreach (Condition c in pConditions)
            {
                c.SetParent(condition);
                condition.childConditions.Add(c);
            }

            return condition;
        }

        /// <summary>
        /// Represents an instance of Condition with internal parameters in AND operation
        /// </summary>
        /// <param name="pRP1">First parameter</param>
        /// <param name="pRP2">Second parameter</param>
        /// <param name="pParameters">List of parameters</param>
        /// <returns>Condition instance</returns>
        public static Condition AND(RecordParameter pRP1, params RecordParameter[] pParameters)
        {
            Condition condition = new Condition();
            condition.TypeOfCondition = ConditionType.And;
            condition.TypeOfChildConditions = ConditionChildType.Parameters;

            pRP1.OwnerCondition = condition;
            condition.childParameters.Add(pRP1);
            foreach (RecordParameter c in pParameters)
            {
                condition.childParameters.Add(c);
            }
            return condition;
        }

        /// <summary>
        /// Represents an instance of Condition with internal parameters in OR operation
        /// </summary>
        /// <param name="pC1">First condition</param>
        /// <param name="pC2">Second condition</param>
        /// <param name="pConditions">List of conditions</param>
        /// <returns>Condition instance</returns>
        public static Condition OR(Condition pC1, params Condition[] pConditions)
        {
            Condition condition = new Condition();
            condition.TypeOfCondition = ConditionType.Or;
            condition.TypeOfChildConditions = ConditionChildType.Conditions;

            pC1.SetParent(condition);
            condition.childConditions.Add(pC1);
            foreach (Condition c in pConditions)
            {
                c.SetParent(condition);
                condition.childConditions.Add(c);
            }
            return condition;
        }

        /// <summary>
        /// Represents an instance of Condition with internal parameters in OR operation
        /// </summary>
        /// <param name="pRP1">First parameter</param>
        /// <param name="pRP2">Second parameter</param>
        /// <param name="pParameters">List of parameters</param>
        /// <returns>Condition instance</returns>
        public static Condition OR(RecordParameter pRP1, params RecordParameter[] pParameters)
        {
            Condition condition = new Condition();
            condition.TypeOfCondition = ConditionType.Or;
            condition.TypeOfChildConditions = ConditionChildType.Parameters;

            pRP1.OwnerCondition = condition;
            condition.childParameters.Add(pRP1);
            foreach (RecordParameter c in pParameters)
            {
                c.OwnerCondition = condition;
                condition.childParameters.Add(c);
            }
            return condition;
        }

        /// <summary>
        /// Entry point for building a query's where condition
        /// </summary>
        /// <param name="listParameters">List empty or full of parameters</param>
        /// <returns>String</returns>
        public String Build(List<RecordParameter> listParameters)
        {
            int start = 0;
            return this.Build(null, ref start, listParameters);
        }

        /// <summary>
        /// Entry point for building a query's where condition
        /// </summary>
        /// <param name="obj">not in use...</param>
        /// <param name="startupIndexParameter">index of first parameter</param>
        /// <param name="listParameters">List empty or full of parameters</param>
        /// <returns>String</returns>
        internal String Build(Object obj, ref int startupIndexParameter, List<RecordParameter> listParameters)
        {
            if (this.IsEmpty)
                return "(1=1)";

            String result = "";
            String temporaryStartFormat = "({0})";

            if (this.TypeOfCondition == ConditionType.None)
                return String.Empty;

            if (this.TypeOfChildConditions == ConditionChildType.None)
                return String.Empty;

            if (listParameters == null)
                listParameters = new List<RecordParameter>();

            if (this.TypeOfCondition == ConditionType.And)
            {
                if (this.TypeOfChildConditions == ConditionChildType.Conditions)
                {
                    String composeFormat = "";
                    List<String> cs = new List<string>();
                    for (int i = 0; i < childConditions.Count; i++)
                    {
                        composeFormat += " AND {" + i + "}";
                        cs.Add(childConditions[i].Build(null, ref startupIndexParameter, listParameters));
                    }
                    result = String.Format(composeFormat.Substring(5), cs.ToArray());
                }
                if (this.TypeOfChildConditions == ConditionChildType.Parameters)
                {
                    result = this.ComposeParametersFormat(this.TypeOfCondition, childParameters, ref startupIndexParameter, listParameters);
                }
            }
            if (this.TypeOfCondition == ConditionType.Or)
            {
                if (this.TypeOfChildConditions == ConditionChildType.Conditions)
                {
                    String composeFormat = "";
                    List<String> cs = new List<string>();
                    for (int i = 0; i < childConditions.Count; i++)
                    {
                        composeFormat += " OR {" + i + "}";
                        cs.Add(childConditions[i].Build(null, ref startupIndexParameter, listParameters));
                    }
                    result = String.Format(composeFormat.Substring(4), cs.ToArray());
                }
                if (this.TypeOfChildConditions == ConditionChildType.Parameters)
                {
                    result = this.ComposeParametersFormat(this.TypeOfCondition, childParameters, ref startupIndexParameter, listParameters);
                }
            }
            return String.Format(temporaryStartFormat, result);
        }

        internal void SetParent(Condition pParent)
        {
            parent = pParent;
        }

        /// <summary>
        /// Compose a String Where result with list of parameters
        /// </summary>
        /// <param name="conditionType">Condition Type</param>
        /// <param name="childs">Parameters</param>
        /// <param name="startupIndexParameter">Index current parameters</param>
        /// <param name="listParameters">List output of parameters</param>
        /// <returns>String Format Where condition</returns>
        protected String ComposeParametersFormat(ConditionType conditionType, List<RecordParameter> childs, ref int startupIndexParameter, List<RecordParameter> listParameters)
        {
            String result = String.Empty;
            String composeFormat = String.Empty;
            String Operation = conditionType == ConditionType.And ? "AND" : "OR";
            List<String> cs = new List<string>();
            for (int i = 0; i < childParameters.Count; i++)
            {
                if (childParameters[i].OwnerParameter.ParameterName != String.Format("p{0}", startupIndexParameter))
                {
                    childParameters[i].ParameterField = childParameters[i].ParameterName.Clone() as String;
                    childParameters[i].ParameterName = String.Format("p{0}", startupIndexParameter);
                    childParameters[i].OwnerParameter.ParameterName = String.Format("p{0}", startupIndexParameter);
                }
                String singlePar = CompareKindToString.SignWithFormat(childParameters[i], childParameters[i].CompareKindExpression);

                composeFormat += " " + Operation + " {" + i + "}";
                cs.Add(singlePar);
                startupIndexParameter++;
                if (CompareKindToString.IsSignWithAddingParameter(childParameters[i].CompareKindExpression))
                    listParameters.Add(childParameters[i]);
            }
            if (conditionType == ConditionType.Or)
                result = String.Format(composeFormat.Substring(4), cs.ToArray());
            if (conditionType == ConditionType.And)
                result = String.Format(composeFormat.Substring(5), cs.ToArray());
            return result;
        }
    }
}