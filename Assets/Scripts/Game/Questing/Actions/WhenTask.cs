// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2017 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Gavin Clayton (interkarma@dfworkshop.net)
// Contributors:    
// 
// Notes:
//

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System;

namespace DaggerfallWorkshop.Game.Questing
{
    /// <summary>
    /// Handles a when|when not task performed condition chain.
    /// </summary>
    public class WhenTask : ActionTemplate
    {
        List<Evaluation> evaluations = new List<Evaluation>();

        struct Evaluation
        {
            public string task;
            public Operator op;
        }

        enum Operator
        {
            Nothing,
            When,
            WhenNot,
            And,
            AndNot,
            Or,
            OrNot,
        }

        public override string Pattern
        {
            // Initial match only looks for opening "when not task"|"when task"
            get { return @"when not (?<taskName>[a-zA-Z0-9_.]+)|when (?<taskName>[a-zA-Z0-9_.]+)"; }
        }

        public WhenTask(Quest parentQuest)
            : base(parentQuest)
        {
            IsTriggerCondition = true;
        }

        public override IQuestAction CreateNew(string source, Quest parentQuest)
        {
            base.CreateNew(source, parentQuest);

            // Full pattern to get chain of evaluations
            string fullPattern = @"when not (?<taskName>[a-zA-Z0-9_.]+)|when (?<taskName>[a-zA-Z0-9_.]+)|and not (?<taskName>[a-zA-Z0-9_.]+)|and (?<taskName>[a-zA-Z0-9_.]+)|or not (?<taskName>[a-zA-Z0-9_.]+)|or (?<taskName>[a-zA-Z0-9_.]+)";

            // Split conditions
            MatchCollection matches = Regex.Matches(source, fullPattern);
            if (matches.Count == 0)
                return null;

            // Factory new WhenTask
            WhenTask condition = new WhenTask(parentQuest);
            condition.BuildEvals(matches);

            return condition;
        }

        public override bool CheckTrigger(Task caller)
        {
            return CheckEvals();
        }

        /// <summary>
        /// Construct evaluation chain from matches.
        /// </summary>
        void BuildEvals(MatchCollection matches)
        {
            for (int i = 0; i < matches.Count; i++)
            {
                Evaluation eval = new Evaluation();

                // Get task name
                eval.task = matches[i].Groups["taskName"].Value;

                // Get operator
                string matchValue = matches[i].Value;
                if (matchValue.Contains("when not"))
                    eval.op = Operator.WhenNot;
                else if (matchValue.Contains("and not"))
                    eval.op = Operator.AndNot;
                else if (matchValue.Contains("or not"))
                    eval.op = Operator.OrNot;
                else if (matchValue.Contains("when"))
                    eval.op = Operator.When;
                else if (matchValue.Contains("and"))
                    eval.op = Operator.And;
                else if (matchValue.Contains("or"))
                    eval.op = Operator.Or;
                else
                    throw new Exception("Operator not found in WhenTask condition.");

                evaluations.Add(eval);
            }
        }

        /// <summary>
        /// Checks evaluation chain built at factory time.
        /// Assuming simple left-to-right short-circuit evaluation.
        /// </summary>
        /// <returns>Result of chained evaluations.</returns>
        bool CheckEvals()
        {
            // Must have at least one evaluation by this point
            if (evaluations.Count == 0)
                throw new Exception("WhenTask condition has no evaluations.");

            // Evaluate conditions left to right
            bool left = false, right = false;
            for (int i = 0; i < evaluations.Count; i++)
            {
                // Get task state based on operator
                Operator op = evaluations[i].op;
                Symbol taskSymbol = new Symbol(evaluations[i].task);
                switch(op)
                {
                    case Operator.When:
                    case Operator.And:
                    case Operator.Or:
                        right = IsTaskSet(taskSymbol);
                        break;

                    case Operator.WhenNot:
                    case Operator.AndNot:
                    case Operator.OrNot:
                        right = !IsTaskSet(taskSymbol);
                        break;

                    default:
                        return false;
                }

                // Evaluate short-circuit conditions
                switch (op)
                {
                    // Handle single-task condition
                    case Operator.When:
                    case Operator.WhenNot:
                        if (evaluations.Count == 1 && !right)
                            return false;
                        break;

                    // Both sides must be true
                    case Operator.And:
                    case Operator.AndNot:
                        if (!left || !right)
                            return false;
                        break;

                    // Either side can be true to result as true
                    case Operator.Or:
                    case Operator.OrNot:
                        if (!left && !right)
                            return false;
                        else
                            right = true;
                        break;

                    default:
                        return false;
                }

                // Move to the right
                left = right;
            }

            // All conditions have evaluated
            return true;
        }

        /// <summary>
        /// Checks is a task is set.
        /// </summary>
        bool IsTaskSet(Symbol symbol)
        {
            Task task = ParentQuest.GetTask(symbol);
            if (task == null)
            {
                Debug.LogErrorFormat("Task/Variable not found '{0}'", Symbol.Name);
                return false;
            }

            return task.IsSet;
        }
    }
}