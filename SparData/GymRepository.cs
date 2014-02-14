using NHibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SparModel;

namespace SparData
{
	public class GymRepository
	{
		private static ISession getSession()
		{
			return SessionProvider.SessionFactory.OpenSession();
		}

		public Gym GetGymById(int gymId)
		{
			Gym gym = null;
			using (var session = getSession())
			{
				gym = session.Get<Gym>(gymId);
			}

			return gym;
		}

		public void SaveGym(Gym gym)
		{
			using (var session = getSession())
			{
				using (var transaction = session.BeginTransaction())
				{
					if (gym.Id == null)
						gym.InsertDate = gym.UpdateDate = DateTime.Now;
					else
						gym.UpdateDate = DateTime.Now;

					session.SaveOrUpdate(gym);
					transaction.Commit();
				}
			}
		}
	}
}
