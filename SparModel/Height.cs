using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SparModel
{
	public class Height
	{
		public static Dictionary<double, string> HeightToCentimetersMap = new Dictionary<double, string>() {
			{142, "4'8\""},
			{144.5, "4'9\""},
			{147, "4'10\""},
			{150, "4'11\""},
			{152.5, "5'"},
		};

		private double m_HeightValue = 0;
		public double HeightValue 
		{ 
			get { return m_HeightValue; }
			set 
			{
				HeightLabel = "";
				if (HeightToCentimetersMap[value] != null)
					HeightLabel = HeightToCentimetersMap[value];

				m_HeightValue = value;
			}
		}
		public string HeightLabel { get; private set; }
	}
}
