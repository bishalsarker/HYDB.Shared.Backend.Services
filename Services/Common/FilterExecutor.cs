using System;
using System.Collections.Generic;
using System.Text;
using HYDB.FilterEngine;
using HYDB.Services.Models;

namespace HYDB.Services.Common.Services
{
    public static class FilterExecutor
    {
        public static bool Execute(string expression,
                                   string dataObjectId,
                                   List<DataObjectKeyValue> dataObjKeyValues,
                                   List<DataModelProperty> dataModelProps,
                                   dynamic args)
        {
            var valueDict = BuildValueDictionary(dataObjectId, dataObjKeyValues, dataModelProps, args);
            List<Clause> clauseList = ParseFilterExpression(expression, valueDict);
            return Interpreter.Execute(clauseList);
        }

        private static Dictionary<string, dynamic> BuildValueDictionary(string dataObjectId,
                                                                        List<DataObjectKeyValue> dataObjKeyValues,
                                                                        List<DataModelProperty> dataModelProps,
                                                                        dynamic args)
        {
            var dict = new Dictionary<string, dynamic>();

            dict.Add("id", dataObjectId);

            foreach (var obj in dataObjKeyValues)
            {
                var prop = dataModelProps.Find(x => x.Id.ToString() == obj.KeyString);
                if (prop != null)
                {
                    dict.Add(prop.Name, Convert.ChangeType(obj.Value, PropertyTypeResolver.Resolve(prop.Type)));
                }
            }

            if (args != null)
            {
                foreach (var arg in args)
                {
                    dict.Add(arg.Key, arg.Value);
                }
            }

            return dict;
        }

        private static dynamic GetValueFromDictionary(string key, Dictionary<string, dynamic> valueDictionary)
        {
            dynamic value;
            if (valueDictionary.ContainsKey(key))
            {
                if (valueDictionary.TryGetValue(key, out value)) { }
                return value;
            }
            else
            {
                return null;
            }
        }

        private static List<Clause> ParseFilterExpression(string where, Dictionary<string, dynamic> valueDictionary)
        {
            var parsedClause = Tokenizr.ParseClauses(where);
            var newClauseList = new List<Clause>();
            foreach (var clause in parsedClause)
            {
                var resolvedVaribale = new List<Variable>();
                foreach (var variable in clause.Logic.Definitions)
                {
                    if (variable.Name != "const")
                    {
                        var value = GetValueFromDictionary(variable.Name, valueDictionary);
                        if (value != null)
                        {
                            resolvedVaribale.Add(new Variable()
                            {
                                Name = variable.Name,
                                Value = value,
                                Type = value.GetType()
                            });
                        }
                        else
                        {
                            resolvedVaribale.Add(new Variable()
                            {
                                Name = variable.Name,
                                Value = 0,
                                Type = typeof(int)
                            });
                        }
                    }
                    else
                    {
                        resolvedVaribale.Add(variable);
                    }
                }

                var newClause = new Clause()
                {
                    Logic = clause.Logic,
                    RightConjuction = clause.RightConjuction
                };

                newClause.Logic.Definitions = resolvedVaribale;
                newClauseList.Add(newClause);
            }

            return newClauseList;
        }
    }
}
