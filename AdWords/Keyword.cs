﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace g2b
{
    using Google.Api.Ads.AdWords.Lib;
    using Google.Api.Ads.AdWords.v201509;

    partial class AdWords
    {
        public class Listing
        {
            public Keyword Keyword
            {
                get { return Ac.criterion as Keyword; }
            }

            public BiddableAdGroupCriterion Ac;
        }
        private static readonly Lazy<AdGroupCriterionService> KeywordService =
            new Lazy<AdGroupCriterionService>(
                () => (AdGroupCriterionService)User.Value.GetService(AdWordsService.v201509.CampaignService));

        //ad group id lost in Keyword
        public static IEnumerable<Listing> GetKeywords(IEnumerable<long> agids)
        {
            var str = agids.Select(i => i.ToString()).ToArray();
            var sel0 = new Selector
            {
                fields = new[] {"Id", "KeywordText", "KeywordMatchType", "CpcBid", "Status", "AdGroupId"},
                predicates =
                    new[]
                    {
                        new Predicate
                        {
                            field = "AdGroupId",
                            @operator = PredicateOperator.IN,
                            operatorSpecified = true,
                            values = str
                        }
                    }
            };
            return
                from c in KeywordService.Value.get(sel0).entries
                let ac = c as BiddableAdGroupCriterion
                where ac != null
                let k = c.criterion as Keyword
                where k != null
                select new Listing {Ac = ac};
        }

        public static AdGroupCriterionReturnValue SetKeywords(IEnumerable<Listing> cs)
        {
            return KeywordService.Value.mutate(
                cs.Select(
                    c =>
                        new AdGroupCriterionOperation
                        {
                            operand = c.Ac,
                            @operator = Operator.SET
                        }).ToArray());
        }

        public static AdGroupCriterionReturnValue AddKeywords(IList<Listing> cs)
        {
            return KeywordService.Value.mutate(
                cs.Select(c => new AdGroupCriterionOperation
                {
                    operand = c.Ac,
                    @operator = Operator.ADD
                }).ToArray());
        }

    }
}
