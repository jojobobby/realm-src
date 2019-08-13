﻿using System.Collections.Generic;
using System.Collections.Specialized;
using Anna.Request;
using common;

namespace server.@char
{
    class list : RequestHandler
    {
        private static List<ServerItem> GetServerList()
        {
            var ret = new List<ServerItem>();
            foreach (var server in Program.ISManager.GetServerList())
            {
                ret.Add(new ServerItem()
                {
                    Name = server.name,
                    Lat = server.latitude,
                    Long = server.longitude,
                    Port = server.port,
                    DNS = server.address,
                    Usage = server.players / (float)server.maxPlayers,
                    AdminOnly = server.adminOnly
                });
            }
            return ret;
        }

        public override void HandleRequest(RequestContext context, NameValueCollection query)
        {
            DbAccount acc;
            var status = Database.Verify(query["guid"], query["password"], out acc);
            if (status == LoginStatus.OK || status == LoginStatus.AccountNotExists)
            {
                if (status == LoginStatus.AccountNotExists)
                    acc = Database.CreateGuestAccount(query["guid"]);

                var list = CharList.FromDb(Database, acc);
                list.Servers = GetServerList();
                WriteXml(context, list.ToXml().ToString());
            }
            else
                Write(context, "<Error>" + status.GetInfo() + "</Error>");
        }
    }
}