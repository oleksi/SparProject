using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SparModel;
using SparData;
using Microsoft.AspNet.Identity;

namespace TestConsoleClient
{
	class Program
	{
		static void Main(string[] args)
		{
			//SessionProvider.RebuildSchema();

			//GymRepository gymRepo = new GymRepository();
			////Gym gleasonsGym = new Gym() { Name = "Gleason's Gym", StreetAddress = "77 Front St", City = "Brooklyn", State = "NY", ZipCode = "11201", Phone = "(718) 797-2872" };
			////gymRepo.SaveGym(gleasonsGym);
			//Gym gleasonsGym = gymRepo.GetGymById(1);

			FighterRepository fighterRepo = new FighterRepository();
			Fighter fighter = fighterRepo.GetFighterByIdentityUserId("bc625cdf-371b-4599-9783-480ce472f995");
			//Fighter fighter = fighterRepo.GetFighterById(9);
			//Fighter newFighter = new Fighter() { Name = "Oleksiy Shevchukevych", Email="cheetah@gmail.com", Sex = true, DateOfBirth = DateTime.Parse("4/19/1978"), Height = 5.10, Weight = 176, NumberOfFights = 3, Gym = gleasonsGym };
			//fighterRepo.SaveFighter(newFighter);

			//SparIdentityUserStore<SparIdentityUser> store = new SparIdentityUserStore<SparIdentityUser>();
			//SparIdentityUser user = store.FindByIdAsync("89c2eba2-2e52-4a84-9dd4-417538b3b9c0").Result;

			//store.RemoveLoginAsync(user, new UserLoginInfo("gggggg", "hhhhhhhh"));

			//user.Logins.Add(new SparUserLoginInfo() { UserLoginInfoIdentifier = new UserLoginInfoIdentifier() { UserId = user.Id, LoginProvider = "gggggg", ProviderKey = "hhhhhhhh" } });
			//IList<UserLoginInfo> list = store.GetLoginsAsync(user).Result;

			//store.UpdateAsync(user);
		}
	}
}
