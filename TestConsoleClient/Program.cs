﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SparModel;
using SparData;

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

			//FighterRepository fighterRepo = new FighterRepository();
			//Fighter newFighter = new Fighter() { Name = "Oleksiy Shevchukevych", Email="cheetah@gmail.com", Sex = true, DateOfBirth = DateTime.Parse("4/19/1978"), Height = 5.10, Weight = 176, NumberOfFights = 3, Gym = gleasonsGym };
			//fighterRepo.SaveFighter(newFighter);

			SparIdentityUserStore<SparIdentityUser> store = new SparIdentityUserStore<SparIdentityUser>();
			//SparIdentityUser user = store.FindByIdAsync("89c2eba2-2e52-4a84-9dd4-417538b3b9c0").Result;

			//user.Login = new SparUserLoginInfo() { UserId = user.Id, LoginProvider = "eeeeeee", ProviderKey = "fffff" };
			//store.UpdateAsync(user);
			SparIdentityUser user = store.FindAsync(new Microsoft.AspNet.Identity.UserLoginInfo("eeeeeee", "fffff")).Result;
		}
	}
}
