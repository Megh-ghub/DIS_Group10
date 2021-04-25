using DIS_Group10.Data;
using DIS_Group10.Models;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace DIS_Group10.Data
{
    public static class DbInitializer
    {
        static HttpClient httpClient;
        static string BASE_URL = "https://developer.nps.gov/api/v1";
        static string API_KEY = "VXU1saXu5sQvPJawH28GDeoVR8nT6C56vmxGiBFB";
        public static void Initialize(ApplicationDbContext context)
        {
            context.Database.EnsureCreated();
            getStates(context);

            getActivities(context);
            getParks(context);
        }

// Get:: Park Data :: /parks

        public static void getParks(ApplicationDbContext context)
        {

            if (context.Parks.Any())
            {
                return;
            }

            string uri = BASE_URL + "/parks?limit=100";
            string responsebody = "";
            httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.Add("X-Api-Key", API_KEY);
            httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            httpClient.BaseAddress = new Uri(uri);

            try
            {
                HttpResponseMessage response = httpClient.GetAsync(uri).GetAwaiter().GetResult();

                if (response.IsSuccessStatusCode)
                {
                    responsebody = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                }

                if (!responsebody.Equals(""))
                {
                    JObject parsedResponse = JObject.Parse(responsebody);
                    JArray parks = (JArray)parsedResponse["data"];

                    foreach (JObject jsonpark in parks)
                    {
                        Park p = new Park
                        {
                            ID = (string)jsonpark["id"],
                            url = (string)jsonpark["url"],
                            fullName = (string)jsonpark["fullName"],
                            parkCode = (string)jsonpark["parkCode"],
                            description = (string)jsonpark["description"],
                        };
                        context.Parks.Add(p);
                        string[] states = ((string)jsonpark["states"]).Split(",");
                        foreach (string s in states)
                        {
                            State st = context.States.Where(c => c.ID == s).FirstOrDefault();
                            if (st != null)
                            {
                                StatePark sp = new StatePark()
                                {
                                    state = st,
                                    park = p
                                };
                                context.StateParks.Add(sp);
                                context.SaveChanges();
                            }
                        }
                        JArray activities = (JArray)jsonpark["activities"];
                        if (activities.Count != 0)
                        {
                            foreach (JObject jsonactivity in activities)
                            {
                                Activity a = context.Activities.Where(c => c.ID == (string)jsonactivity["id"]).FirstOrDefault();
                                if (a == null)
                                {
                                    a = new Activity
                                    {
                                        ID = (string)jsonactivity["id"],
                                        name = (string)jsonactivity["name"]
                                    };
                                    context.Activities.Add(a);
                                    context.SaveChanges();
                                }
                                ParkActivity pa = new ParkActivity
                                {
                                    activity = a,
                                    park = p
                                };
                                context.ParkActivities.Add(pa);
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

// Get:: Activity Data :: /activities

        public static void getActivities(ApplicationDbContext context)
        {
            if (context.Activities.Any())
            {
                return;
            }

            string uri = BASE_URL + "/activities?limit=100";
            string responsebody = "";

            httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.Add("X-Api-Key", API_KEY);
            httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

            httpClient.BaseAddress = new Uri(uri);

            try
            {
                HttpResponseMessage response = httpClient.GetAsync(uri).GetAwaiter().GetResult();

                if (response.IsSuccessStatusCode)
                {
                    responsebody = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                }

                if (!responsebody.Equals(""))
                {
                    JObject parsedResponse = JObject.Parse(responsebody);
                    JArray activities = (JArray)parsedResponse["data"];
                    foreach (JObject jsonactivity in activities)
                    {
                        Activity a = new Activity
                        {
                            ID = (string)jsonactivity["id"],
                            name = (string)jsonactivity["name"]
                        };
                        context.Activities.Add(a);
                    }
                    context.SaveChanges();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

// Force Persisting State Code vs State Name for Clean Display Only

        public static void getStates(ApplicationDbContext context)
        {
            context.Database.EnsureCreated();
            State[] stlist = new State[]
            {
                new State{ID="AL",name="Alabama"},
                new State{ID="AK",name="Alaska"},
                new State{ID="AZ",name="Arizona"},
                new State{ID="AR",name="Arkansas"},
                new State{ID="CA",name="California"},
                new State{ID="CO",name="Colorado"},
                new State{ID="DE",name="Delaware"},
                new State{ID="DC",name="District of Columbia"},
                new State{ID="FL",name="Florida"},
                new State{ID="GA",name="Georgia"},
                new State{ID="HI",name="Hawaii"},
                new State{ID="ID",name="Idaho"},
                new State{ID="IL",name="Illinois"},
                new State{ID="IN",name="Indiana"},
                new State{ID="IA",name="Iowa"},
                new State{ID="KS",name="Kansas"},
                new State{ID="KY",name="Kentucky"},
                new State{ID="LA",name="Louisiana"},
                new State{ID="ME",name="Maine"},
                new State{ID="MD",name="Maryland"},
                new State{ID="MA",name="Massachusetts"},
                new State{ID="MI",name="Michigan"},
                new State{ID="MN",name="Minnesota"},
                new State{ID="MS",name="Mississippi"},
                new State{ID="MO",name="Missouri"},
                new State{ID="MT",name="Montana"},
                new State{ID="NE",name="Nebraska"},
                new State{ID="NV",name="Nevada"},
                new State{ID="NH",name="New Hampshire"},
                new State{ID="NJ",name="New Jersey"},
                new State{ID="NM",name="New Mexico"},
                new State{ID="NY",name="New York"},
                new State{ID="NC",name="North Carolina"},
                new State{ID="ND",name="North Dakota"},
                new State{ID="OH",name="Ohio"},
                new State{ID="OK",name="Oklahoma"},
                new State{ID="OR",name="Oregon"},
                new State{ID="PA",name="Pennsylvania"},
                new State{ID="RI",name="Rhode Island"},
                new State{ID="SC",name="South Carolina"},
                new State{ID="SD",name="South Dakota"},
                new State{ID="TN",name="Tennessee"},
                new State{ID="TX",name="Texas"},
                new State{ID="UT",name="Utah"},
                new State{ID="VT",name="Vermont"},
                new State{ID="VA",name="Virginia"},
                new State{ID="WA",name="Washington"},
                new State{ID="WV",name="West Virginia"},
                new State{ID="WI",name="Wisconsin"},
                new State{ID="WY",name="Wyoming"},
            };
            try
            {
                if (!context.States.Any())
                {
                    foreach (State o in stlist)
                    {
                        context.States.Add(o);
                    }
                    context.SaveChanges();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

    }
}