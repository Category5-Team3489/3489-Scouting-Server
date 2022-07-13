#pragma warning disable IDE1006 // Naming Styles

using System;
using System.Text;

namespace ScoutingShared3489
{
    public abstract class TBASchema
    {
        public void PrintProperties(int indentionLevel = 0)
        {
            var type = GetType();
            var properties = type.GetProperties();

            StringBuilder indention = new StringBuilder();
            for (int i = 0; i < indentionLevel; i++)
            {
                indention.Append("   ");
            }
            string indentionText = indention.ToString();

            Console.WriteLine($"{indentionText}{type.Name}");

            int number = 1;

            foreach (var property in properties)
            {
                string name = property.Name;
                var propertyType = property.PropertyType;

                var value = property.GetValue(this);
                if (value is null)
                {
                    Console.WriteLine($"{indentionText}{number++}. {name}: {propertyType.Name} (null)");
                }
                else if (propertyType.IsArray)
                {
                    var elementType = propertyType.GetElementType();
                    var array = (object[])value;
                    Console.WriteLine($"{indentionText}{number++}. {name}: {elementType.Name} (array)");
                    int elementNumber = 1;
                    foreach (object element in array)
                    {
                        Console.WriteLine($"   {indentionText}{elementNumber}.");
                        if (element is null)
                        {
                            Console.WriteLine($"      {indentionText}(null)");
                        }
                        else if (typeof(TBASchema).IsAssignableFrom(element.GetType()))
                        {
                            ((TBASchema)element).PrintProperties(indentionLevel + 2);
                        }
                        else
                        {
                            Console.WriteLine($"      {indentionText}{element}");
                        }
                        elementNumber++;
                    }
                }
                else if (typeof(TBASchema).IsAssignableFrom(propertyType))
                {
                    Console.WriteLine($"{indentionText}{number++}. {name}: {propertyType.Name} (printable)");
                    ((TBASchema)value).PrintProperties(indentionLevel + 1);
                }
                else
                {
                    Console.WriteLine($"{indentionText}{number++}. {name}: {propertyType.Name} = {value}");
                }
            }
        }
    }

    public class MatchSimpleSchema : TBASchema
    {
        public class MatchAllianceSchema : TBASchema
        {
            public int score { get; set; }
            public string[] team_keys { get; set; }
            public string[] surrogate_team_keys { get; set; }
            public string[] dq_team_keys { get; set; }
        }

        public class AlliancesSchema : TBASchema
        {
            public MatchAllianceSchema red { get; set; }
            public MatchAllianceSchema blue { get; set; }
        }

        public string key { get; set; }
        public string comp_level { get; set; }
        public int set_number { get; set; }
        public int match_number { get; set; }
        public AlliancesSchema alliances { get; set; }
        public string winning_alliance { get; set; }
        public string event_key { get; set; }
        public int time { get; set; }
        public int predicted_time { get; set; }
        public int actual_time { get; set; }
    }

    public class TeamSimpleSchema : TBASchema
    {
        public string key { get; set; }
        public int team_number { get; set; }
        public string nickname { get; set; }
        public string name { get; set; }
        public string city { get; set; }
        public string state_prov { get; set; }
        public string country { get; set; }
    }

}

#pragma warning restore IDE1006 // Naming Styles