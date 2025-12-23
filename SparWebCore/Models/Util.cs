using System.Collections.Generic;

namespace SparWebCore.Models
{
    public partial class Util
    {
        public static Dictionary<double, string> HeightToCentimetersMap = new Dictionary<double, string>() 
        {
            {1, "less than 4'8\""},
            {142, "4'8\""},
            {144.5, "4'9\""},
            {147, "4'10\""},
            {150, "4'11\""},
            {152.5, "5'"},
            {155, "5'1\""},
            {157.5, "5'2\""},
            {160, "5'3\""},
            {162.5, "5'4\""},
            {165, "5'5\""},
            {167.5, "5'6\""},
            {170, "5'7\""},
            {172.5, "5'8\""},
            {175, "5'9\""},
            {177.5, "5'10\""},
            {180, "5'11\""},
            {183, "6'"},
            {185.5, "6'1\""},
            {188, "6'2\""},
            {190.5, "6'3\""},
            {1000, "greater than 6'3\""}
        };

        public static Dictionary<double, string> WeightClassMap = new Dictionary<double, string>()
        {
            {106, "below 106 lbs: Light Flyweight"},
            {112, "112 lbs: Flyweight"},
            {119, "119 lbs: Bantamweight"},
            {125, "125 lbs: Featherweight"},
            {132, "132 lbs: Lightweight"},
            {141, "141 lbs: Light Welterweight"},
            {152, "152 lbs: Welterweight"},
            {165, "165 lbs: Middleweight"},
            {178, "178 lbs: Light Heavyweight"},
            {201, "201 lbs: Heavyweight"},
            {1000, "over 201 lbs: Super Heavyweight"}
        };

        public static Dictionary<string, string> States = new Dictionary<string, string>()
        {
            {"AL", "Alabama"},
            {"AK", "Alaska"},
            {"AB", "Alberta"},
            {"AZ", "Arizona"},
            {"AR", "Arkansas"},
            {"BC", "British Columbia"},
            {"CA", "California"},
            {"CO", "Colorado"},
            {"CT", "Connecticut"},
            {"DC", "District of Columbia"},
            {"DE", "Delaware"},
            {"FL", "Florida"},
            {"GA", "Georgia"},
            {"HI", "Hawaii"},
            {"ID", "Idaho"},
            {"IL", "Illinois"},
            {"IN", "Indiana"},
            {"IA", "Iowa"},
            {"KS", "Kansas"},
            {"KY", "Kentucky"},
            {"LA", "Louisiana"},
            {"ME", "Maine"},
            {"MB", "Manitoba"},
            {"MD", "Maryland"},
            {"MA", "Massachusetts"},
            {"MI", "Michigan"},
            {"MN", "Minnesota"},
            {"MS", "Mississippi"},
            {"MO", "Missouri"},
            {"MT", "Montana"},
            {"NE", "Nebraska"},
            {"NV", "Nevada"},
            {"NL", "Newfoundland and Labrador"},
            {"NB", "New Brunswick"},
            {"NH", "New Hampshire"},
            {"NJ", "New Jersey"},
            {"NM", "New Mexico"},
            {"NY", "New York"},
            {"NC", "North Carolina"},
            {"ND", "North Dakota"},
            {"NS", "Nova Scotia"},
            {"OH", "Ohio"},
            {"OK", "Oklahoma"},
            {"ON", "Ontario"},
            {"OR", "Oregon"},
            {"PA", "Pennsylvania"},
            {"PE", "Prince Edward Island"},
            {"QC", "Quebec"},
            {"RI", "Rhode Island"},
            {"SK", "Saskatchewan"},
            {"SC", "South Carolina"},
            {"SD", "South Dakota"},
            {"TN", "Tennessee"},
            {"TX", "Texas"},
            {"UT", "Utah"},
            {"VT", "Vermont"},
            {"VA", "Virginia"},
            {"WA", "Washington"},
            {"WV", "West Virginia"},
            {"WI", "Wisconsin"},
            {"WY", "Wyoming"}
        };
    }
}
