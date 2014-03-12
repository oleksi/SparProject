using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SparWeb.Models
{
	public class Util
	{
		public static Dictionary<double, string> HeightToCentimetersMap = new Dictionary<double, string>() {
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
			{1000, "greater than 6'3\""},
		};
	}
}