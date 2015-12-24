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

		public Trainer GetTrainerByStateAndName(string state, string name)
		{
			Trainer trainer = null;
			using (var session = getSession())
			{
				trainer = session.QueryOver<Trainer>().Where(m => m.State == state && m.Name == name.ToLower()).SingleOrDefault();
			}

			return trainer;
		}

		public IList<Trainer> GetAllTrainers()
		{
			IList<Trainer> trainers = new List<Trainer>();
			using (var session = getSession())
			{
				trainers = session.QueryOver<Trainer>().OrderBy(ff => ff.ProfilePictureUploaded).Desc.OrderBy(ff => ff.InsertDate).Desc.JoinQueryOver<SparIdentityUser>(ff => ff.SparIdentityUser).Where(iu => iu.EmailConfirmed == true).List();
			}

			return trainers;
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
