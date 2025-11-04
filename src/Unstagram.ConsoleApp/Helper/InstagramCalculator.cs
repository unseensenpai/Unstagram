using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unstagram.Models.Follower;
using Unstagram.Models.Following;

namespace Unstagram.Helper
{
    public class InstagramCalculator
    {
        public static bool GenerateInformation(string? followingFileName, string? followerFileName)
        {
            try
            {
                if (!string.IsNullOrEmpty(followingFileName) && !string.IsNullOrEmpty(followerFileName))
                {
                    try
                    {
                        string followingJson = File.ReadAllText($"{AppDomain.CurrentDomain.BaseDirectory}{followingFileName}.json");
                        string followerJson = File.ReadAllText($"{AppDomain.CurrentDomain.BaseDirectory}{followerFileName}.json");
                        List<InstagramFollowersModel>? followerModel = JsonConvert.DeserializeObject<List<InstagramFollowersModel>>(followerJson);
                        InstagramFollowingModel? followingModel = JsonConvert.DeserializeObject<InstagramFollowingModel>(followingJson);
                        if (followerModel is not null && followingModel is not null)
                        {
                            List<string> stringListFollower = followerModel.SelectMany(x => x.String_List_Data).Select(x => x.Href).ToList();
                            IEnumerable<string> stringListFollowing = followingModel.Relationships_Following.SelectMany(x => x.String_List_Data).Select(x => x.Href);
                            List<string> imNotFollowingList = stringListFollower.Except(stringListFollowing).ToList();
                            List<string> theyAreNotFollowingList = stringListFollowing.Except(stringListFollower).ToList();
                            string imNotFollowingTextPath = $"{AppDomain.CurrentDomain.BaseDirectory}/Results/YouAreNotFollowingThese.txt";
                            string theyAreNotFollowingTextPath = $"{AppDomain.CurrentDomain.BaseDirectory}/Results/TheseAreNotFollowingYou.txt";
                            string directoryPath = $"{AppDomain.CurrentDomain.BaseDirectory}/Results/";
                            if (!Directory.Exists(directoryPath))
                            {
                                Directory.CreateDirectory(directoryPath);
                            }

                            using (FileStream fileStream = File.Create(imNotFollowingTextPath))
                            {
                                foreach (string? imNot in imNotFollowingList)
                                {
                                    fileStream.Write(Encoding.UTF8.GetBytes(imNot.Replace("https://www.instagram.com/", "") + " \n"));
                                }
                            }

                            using (FileStream fileStream = File.Create(theyAreNotFollowingTextPath))
                            {

                                foreach (string? theyAreNot in theyAreNotFollowingList)
                                {
                                    fileStream.Write(Encoding.UTF8.GetBytes(theyAreNot.Replace("https://www.instagram.com/", "") + " \n"));
                                }
                            }
                            return true;
                        }
                        else
                        {
                            throw new ArgumentNullException("Models are null. Please check for valid json.");
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"An error occured while generating results. ERROR: {ex.Message}", ex);
                        return false;
                    }
                }
                else
                {
                    Console.WriteLine("File names cannot be find in root folder. Copy them to root folder.");
                    return false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occured while generating result files. {ex.Message}");
                return false;
            }
            
        }
    }
}
