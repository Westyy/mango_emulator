using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mango.Communication.Sessions;
using Mango.Utilities;
using Mango.Rooms;
using Mango.Communication.Packets.Outgoing.Navigator;
using MySql.Data.MySqlClient;
using Mango.Database.Exceptions;

namespace Mango.Communication.Packets.Incoming.Navigator
{
    class CreateFlatEvent : IPacketEvent
    {
        public void parse(Session session, ClientPacket packet)
        {
            string RoomName = StringCharFilter.Escape(packet.PopString());
            string ModelName = packet.PopString().ToLower();

            if (RoomName.Length < 3)
            {
                return;
            }

            if (RoomName.Length > 25)
            {
                return;
            }

            RoomModel RoomModel = null;

            if (!Mango.GetServer().GetRoomManager().TryGetModel(ModelName, out RoomModel))
            {
                return;
            }

            if (!RoomModel.IsUsableByPlayer(session.GetPlayer()))
            {
                return;
            }

            // to-do: control maximum rooms per user

            RoomData Data = new RoomData(0, session.GetPlayer().Id, RoomName, "", "", "open",
                "", 0, 25, 0, RoomModel, 1, 0, 1, 0, 0, 0, "landscape=0.0");

            using (var DbCon = Mango.GetServer().GetDatabase().GetConnection())
            {
                try
                {
                    DbCon.Open();
                    DbCon.BeginTransaction();

                    DbCon.SetQuery("INSERT INTO `rooms` (owner_id,name,description,tags,access_type,password,category,max_users,score,model,allow_pets,allow_pets_eating,disable_blocking,hide_walls,thickness_wall,thickness_floor,decorations) VALUES(@ownerid,@name,@desc,@tags,@access,@pass,@category,@maxusers,@score,@model,@allowpets,@allowpetseating,@disableblocking,@hidewalls,@wallthick,@floorthick,@decorations);");

                    string Access = "open";

                    switch (Data.Access)
                    {
                        case RoomAccess.Password_Protected:
                            Access = "password";
                            break;

                        case RoomAccess.Locked:
                            Access = "doorbell";
                            break;
                    }

                    DbCon.AddParameter("ownerid", Data.OwnerId);
                    DbCon.AddParameter("name", Data.Name);
                    DbCon.AddParameter("desc", Data.Description);
                    DbCon.AddParameter("tags", "");
                    DbCon.AddParameter("access", Access);
                    DbCon.AddParameter("pass", "");
                    DbCon.AddParameter("category", Data.CategoryId);
                    DbCon.AddParameter("maxusers", Data.MaxUsers);
                    DbCon.AddParameter("score", Data.Score);
                    DbCon.AddParameter("model", Data.Model.Id);
                    DbCon.AddParameter("allowpets", Data.AllowPets == true ? "1" : "0");
                    DbCon.AddParameter("allowpetseating", Data.AllowPetsEating == true ? "1" : "0");
                    DbCon.AddParameter("disableblocking", Data.DisableRoomBlocking == true ? "1" : "0");
                    DbCon.AddParameter("hidewalls", Data.HideWalls == true ? "1" : "0");
                    DbCon.AddParameter("wallthick", Data.WallThickness);
                    DbCon.AddParameter("floorthick", Data.FloorThickness);
                    DbCon.AddParameter("decorations", "landscape=0.0");
                    DbCon.ExecuteNonQuery();

                    Data.Id = DbCon.SelectLastId();

                    DbCon.Commit();
                }
                catch (DatabaseException)
                {
                    DbCon.Rollback();
                    return;
                }
                catch (MySqlException)
                {
                    DbCon.Rollback();
                    return;
                }
            }

            if (Data.Id > 0)
            {
                session.SendPacket(new FlatCreatedComposer(Data.Id, RoomName));
            }
        }
    }
}
