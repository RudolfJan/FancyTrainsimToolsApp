﻿using Assets.Library.Models;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Xml.XPath;

namespace Assets.Library.Logic
	{
	public class CareerRuleDataAccess
		{
		public static List<CareerRule> ReadRules(XDocument scenarioDoc)
			{
			var ruleList = new List<CareerRule>();
			XElement RulesRoot =
				scenarioDoc.XPathSelectElement("cScenarioProperties/CareerRules/cCareerRules");
			if (RulesRoot != null)
				{
				foreach (var item in RulesRoot.Elements())
					{
					var Temp = item.Name.ToString();
					var ExcludeString = @"Parent,Version,CodeVersion,DLC,StatsGroup,MD5";
					if (!ExcludeString.Contains(Temp))
						{
						var rule = new CareerRule
							{
							TagName = item.Name.ToString(),
							Value = item.Value
							};
						ruleList.Add(rule);
						}
					}
				}

			return ruleList;
				}

			public static void SaveRules(List<CareerRule> ruleList, XDocument scenarioDoc)
				{
				foreach (var Rule in ruleList)
					{
					XElement RuleElement =
						scenarioDoc.XPathSelectElement("cScenarioProperties/CareerRules/cCareerRules/" +
						                               Rule.TagName);
					if (RuleElement != null)
						{
						RuleElement.Value = Rule.Value;
						}
					}
				}
			}
		}



