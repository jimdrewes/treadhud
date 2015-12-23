using System;
using Amazon.DynamoDBv2.DataModel;

namespace JD.TreadHud.Domain.Models
{
    [DynamoDBTableAttribute("TreadHUD-Activities")]
    public class Activity
    {
        [DynamoDBIgnoreAttribute]
        public Guid Id;

        public DateTime StartDate;

        public DateTime? EndDate;

        [DynamoDBHashKeyAttribute("ActivityId")]
        public string ActivityId
        {
            get
            {
                return Id.ToString();
            }

            set
            {
                Id = Guid.Parse(value);
            }
        }

    }
}
