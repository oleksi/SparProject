using NHibernate;
using SparModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SparData
{
	public class TrainerRepository
	{
		private static ISession getSession()
		{
			return SessionProvider.SessionFactory.OpenSession();
		}

		public Trainer GetTrainerById(int trainerId)
		{
			Trainer trainer = null;
			using (var session = getSession())
			{
				trainer = session.Get<Trainer>(trainerId);
			}

			return trainer;
		}

		public Trainer GetTrainerByIdentityUserId(string identityUserId)
		{
			Trainer trainer = null;
			using (var session = getSession())
			{
				trainer = session.QueryOver<Trainer>().Where(m => m.SparIdentityUser.Id == identityUserId).SingleOrDefault();
			}

			return trainer;
		}

		public void SaveTrainer(Trainer trainer)
		{
			using (var session = getSession())
			{
				using (var transaction = session.BeginTransaction())
				{
					if (trainer.Id == null)
						trainer.InsertDate = trainer.UpdateDate = DateTime.Now;
					else
						trainer.UpdateDate = DateTime.Now;

					session.SaveOrUpdate(trainer);
					transaction.Commit();
				}
			}
		}
	}
}
