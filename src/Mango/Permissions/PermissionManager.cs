using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mango.Players;
using Mango.Subscriptions;
using log4net;
using Mango.Database.Exceptions;
using MySql.Data.MySqlClient;

namespace Mango.Permissions
{
    sealed class PermissionManager
    {
        private static readonly ILog log = LogManager.GetLogger("Mango.Permissions.PermissionManager");

        private readonly Dictionary<int, Permission> Permissions = new Dictionary<int, Permission>();

        private readonly Dictionary<int, string> PermissionGroups = new Dictionary<int, string>();

        private readonly Dictionary<int, List<string>> PermissionGroupRights = new Dictionary<int, List<string>>();

        private readonly Dictionary<int, List<string>> PermissionUserRights = new Dictionary<int, List<string>>();

        private readonly Dictionary<int, Dictionary<int, List<string>>> PermissionSubscriptionRights = new Dictionary<int, Dictionary<int, List<string>>>();

        public PermissionManager()
        {
        }

        public void Init()
        {
            using (var DbCon = Mango.GetServer().GetDatabase().GetConnection())
            {
                DbCon.SetQuery("SELECT * FROM `permissions`");
                DbCon.Open();

                using (MySqlDataReader Reader = DbCon.ExecuteReader())
                {
                    while (Reader.Read())
                    {
                        try
                        {
                            this.Permissions.Add(Reader.GetInt32("id"), new Permission(Reader.GetInt32("id"), Reader.GetString("permission"),
                                Reader.GetString("description")));
                        }
                        catch (DatabaseException ex)
                        {
                            log.Error("Unable to load Permission for ID [" + Reader.GetInt32("id") + "]", ex);
                        }
                    }
                }
            }

            using (var DbCon = Mango.GetServer().GetDatabase().GetConnection())
            {
                DbCon.SetQuery("SELECT * FROM `permissions_groups`");
                DbCon.Open();

                using (MySqlDataReader Reader = DbCon.ExecuteReader())
                {
                    while (Reader.Read())
                    {
                        try
                        {
                            this.PermissionGroups.Add(Reader.GetInt32("id"), Reader.GetString("name"));
                        }
                        catch (DatabaseException ex)
                        {
                            log.Error("Unable to load PermissionGroup for ID [" + Reader.GetInt32("id") + "]", ex);
                        }
                    }
                }
            }

            using (var DbCon = Mango.GetServer().GetDatabase().GetConnection())
            {
                DbCon.SetQuery("SELECT * FROM `permissions_rights`");
                DbCon.Open();

                using (MySqlDataReader Reader = DbCon.ExecuteReader())
                {
                    while (Reader.Read())
                    {
                        try
                        {
                            int GroupId = Reader.GetInt32("group_id");
                            int PermissionId = Reader.GetInt32("permission_id");

                            if (!this.PermissionGroups.ContainsKey(GroupId))
                            {
                                continue; // permission group does not exist
                            }

                            Permission Permission = null;

                            if (!this.Permissions.TryGetValue(PermissionId, out Permission))
                            {
                                continue; // permission does not exist
                            }

                            if (PermissionGroupRights.ContainsKey(GroupId))
                            {
                                this.PermissionGroupRights[GroupId].Add(Permission.PermissionName);
                            }
                            else
                            {
                                List<string> RightsSet = new List<string>()
                                {
                                    Permission.PermissionName
                                };

                                this.PermissionGroupRights.Add(GroupId, RightsSet);
                            }
                        }
                        catch (DatabaseException ex)
                        {
                            log.Error("Unable to load PermissionRights for ID [" + Reader.GetInt32("id") + "]", ex);
                        }
                    }
                }
            }

            using (var DbCon = Mango.GetServer().GetDatabase().GetConnection())
            {
                DbCon.SetQuery("SELECT * FROM `permissions_users`");
                DbCon.Open();

                using (MySqlDataReader Reader = DbCon.ExecuteReader())
                {
                    while (Reader.Read())
                    {
                        try
                        {
                            int PermissionId = Reader.GetInt32("permission_id");
                            int UserId = Reader.GetInt32("user_id");

                            Permission Permission = null;

                            if (!this.Permissions.TryGetValue(PermissionId, out Permission))
                            {
                                continue; // permission does not exist
                            }

                            if (this.PermissionUserRights.ContainsKey(UserId))
                            {
                                this.PermissionUserRights[UserId].Add(Permission.PermissionName);
                            }
                            else
                            {
                                List<string> RightsSet = new List<string>()
                                {
                                    Permission.PermissionName
                                };

                                this.PermissionUserRights.Add(UserId, RightsSet);
                            }
                        }
                        catch (DatabaseException ex)
                        {
                            log.Error("Unable to load PermissionUsers for ID [" + Reader.GetInt32("id") + "]", ex);
                        }
                    }
                }
            }

            using (var DbCon = Mango.GetServer().GetDatabase().GetConnection())
            {
                DbCon.SetQuery("SELECT * FROM `permissions_subscriptions`");
                DbCon.Open();

                using (MySqlDataReader Reader = DbCon.ExecuteReader())
                {
                    while (Reader.Read())
                    {
                        try
                        {
                            int PermissionId = Reader.GetInt32("permission_id");
                            int SubscriptionId = Reader.GetInt32("subscription_id");
                            int LevelRequired = Reader.GetInt32("level_required");

                            Permission Permission = null;

                            if (!this.Permissions.TryGetValue(PermissionId, out Permission))
                            {
                                continue; // permission does not exist
                            }

                            if (this.PermissionSubscriptionRights.ContainsKey(SubscriptionId))
                            {
                                Dictionary<int, List<string>> LevelToRights = this.PermissionSubscriptionRights[SubscriptionId];

                                if (!LevelToRights.ContainsKey(LevelRequired))
                                {
                                    LevelToRights.Add(LevelRequired, new List<string>()
                                    {
                                        Permission.PermissionName
                                    });
                                }
                                else
                                {
                                    List<string> RightsList = LevelToRights[LevelRequired];

                                    RightsList.Add(Permission.PermissionName);
                                }
                            }
                            else
                            {
                                List<string> RightsSet = new List<string>()
                                {
                                    Permission.PermissionName
                                };

                                Dictionary<int, List<string>> LevelToRights = new Dictionary<int, List<string>>();

                                LevelToRights.Add(LevelRequired, RightsSet);
                                this.PermissionSubscriptionRights.Add(SubscriptionId, LevelToRights);
                            }
                        }
                        catch (DatabaseException ex)
                        {
                            log.Error("Unable to load PermissionSubscriptions for ID [" + Reader.GetInt32("id") + "]", ex);
                        }
                    }
                }
            }

            /*using (var s = Mango.GetServer().GetDatabaseOld().GetSessionFactory().OpenSession())
            {
                var Perms = s.CreateCriteria<Permission>()
                    .List<Permission>();

                foreach (var Perm in Perms)
                {
                    this.Permissions.Add(Perm.Id, Perm);
                }

                var Groups = s.CreateCriteria<PermissionGroup>()
                    .List<PermissionGroup>();

                foreach (var Group in Groups)
                {
                    this.PermissionGroups.Add(Group.Id, Group.Name);
                }

                var GroupRights = s.CreateCriteria<PermissionRight>()
                    .List<PermissionRight>();

                foreach (var GroupRight in GroupRights)
                {
                    if (!this.PermissionGroups.ContainsKey(GroupRight.GroupId))
                    {
                        continue; // permission group does not exist
                    }

                    Permission Permission = null;

                    if (!this.Permissions.TryGetValue(GroupRight.PermissionId, out Permission))
                    {
                        continue; // permission does not exist
                    }

                    if (PermissionGroupRights.ContainsKey(GroupRight.GroupId))
                    {
                        this.PermissionGroupRights[GroupRight.GroupId].Add(Permission.PermissionName);
                    }
                    else
                    {
                        List<string> RightsSet = new List<string>()
                        {
                            Permission.PermissionName
                        };

                        this.PermissionGroupRights.Add(GroupRight.GroupId, RightsSet);
                    }
                }

                var UserRights = s.CreateCriteria<PermissionUser>()
                    .List<PermissionUser>();

                foreach (var UserRight in UserRights)
                {
                    Permission Permission = null;

                    if (!this.Permissions.TryGetValue(UserRight.PermissionId, out Permission))
                    {
                        continue; // permission does not exist
                    }

                    if (this.PermissionUserRights.ContainsKey(UserRight.UserId))
                    {
                        this.PermissionUserRights[UserRight.UserId].Add(Permission.PermissionName);
                    }
                    else
                    {
                        List<string> RightsSet = new List<string>()
                        {
                            Permission.PermissionName
                        };

                        this.PermissionUserRights.Add(UserRight.UserId, RightsSet);
                    }
                }

                var SubscriptionRights = s.CreateCriteria<PermissionSubscription>()
                    .List<PermissionSubscription>();

                foreach (var SubRight in SubscriptionRights)
                {
                    Permission Permission = null;

                    if (!this.Permissions.TryGetValue(SubRight.PermissionId, out Permission))
                    {
                        continue; // permission does not exist
                    }

                    if (this.PermissionSubscriptionRights.ContainsKey(SubRight.SubscriptionId))
                    {
                        Dictionary<int, List<string>> LevelToRights = this.PermissionSubscriptionRights[SubRight.SubscriptionId];

                        if (!LevelToRights.ContainsKey(SubRight.LevelRequired))
                        {
                            LevelToRights.Add(SubRight.LevelRequired, new List<string>()
                                {
                                    Permission.PermissionName
                                });
                        }
                        else
                        {
                            List<string> RightsList = LevelToRights[SubRight.LevelRequired];

                            RightsList.Add(Permission.PermissionName);
                        }
                    }
                    else
                    {
                        List<string> RightsSet = new List<string>()
                        {
                            Permission.PermissionName
                        };

                        Dictionary<int, List<string>> LevelToRights = new Dictionary<int, List<string>>();

                        LevelToRights.Add(SubRight.LevelRequired, RightsSet);
                        this.PermissionSubscriptionRights.Add(SubRight.SubscriptionId, LevelToRights);
                    }
                }
            }*/

            log.Info("Loaded " + this.Permissions.Count + " permissions.");
            log.Info("Loaded " + this.PermissionGroups.Count + " permissions groups.");
            log.Info("Loaded " + this.PermissionGroupRights.Count + " permissions group rights.");
            log.Info("Loaded " + this.PermissionSubscriptionRights.Count + " permissions subscription rights.");
            log.Info("Loaded " + this.PermissionUserRights.Count + " permission user rights.");
        }

        public List<string> GetPermissionsForPlayer(Player Player)
        {
            List<string> PermissionSet = new List<string>();

            if (this.PermissionUserRights.ContainsKey(Player.Id))
            {
                List<string> UserRights = null;

                if (this.PermissionUserRights.TryGetValue(Player.Id, out UserRights))
                {
                    PermissionSet.AddRange(UserRights);
                }
            }
            else
            {
                int Level = Player.PermissionLevel;

                List<string> PermRights = null;

                if (this.PermissionGroupRights.TryGetValue(Level, out PermRights))
                {
                    PermissionSet.AddRange(PermRights);
                }
            }

            ICollection<Subscription> Subscriptions = Player.GetSubscriptions().Subscriptions;

            List<string> SubPermissionSet = new List<string>();

            foreach (var Subscription in Subscriptions)
            {
                int Id = Subscription.SubscriptionId;
                int Level = Subscription.CurrentLevel;

                Dictionary<int, List<string>> SubLevelRights = null;

                if (this.PermissionSubscriptionRights.TryGetValue(Id, out SubLevelRights))
                {
                    if (SubLevelRights.ContainsKey(Level))
                    {
                        List<string> Rights = SubLevelRights[Level];

                        foreach (string Right in Rights)
                        {
                            if (SubPermissionSet.Contains(Right))
                            {
                                continue;
                            }

                            SubPermissionSet.Add(Right);
                        }
                    }
                }
            }

            List<string> FullPermissionSet = PermissionSet.Union(SubPermissionSet).ToList<string>();

            return FullPermissionSet;
        }
    }
}
