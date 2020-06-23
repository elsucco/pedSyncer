﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using AltV.Net;
using AltV.Net.Elements.Entities;
using AltV.Net.EntitySync;
using AltV.Net.EntitySync.ServerEvent;
using AltV.Net.EntitySync.SpatialPartitions;
using navMesh_Graph_WebAPI;

namespace PedSyncer
{
    internal class PedSyncer : Resource
    {
        private void InitEntitySync()
        {
            AltEntitySync.Init(
                4,
                100,
                (threadCount, repository) => new ServerEventNetworkLayer(threadCount, repository),
                (entity, threadCount) => (entity.Id % threadCount),
                (entityId, entityType, threadCount) => (entityId % threadCount),
                (threadId) => new LimitedGrid3(50_000, 50_000, 100, 10_000, 10_000, 128),
                new IdProvider()
            );

            Console.WriteLine("[INFO] GameEntityResource InitEntitySync startet");
        }

        async public override void OnStart()
        {
            this.InitEntitySync();

            Alt.OnClient<IPlayer, Dictionary<string, string>>("pedSyncer:client:firstSpawn", Controller.OnFirstSpawn);
            Alt.OnClient<IPlayer, object[]>("pedSyncer:client:positions", Controller.OnPositionUpdate);

            Alt.OnPlayerConnect += Controller.OnPlayerConnect;

            AltEntitySync.OnEntityCreate += Controller.OnEntityCreate;
            AltEntitySync.OnEntityRemove += Controller.OnEntityRemove;

            Console.WriteLine("Started");

            NavigationMeshControl.getInstance();
            PedMovementControl.GetInstance();

            Ped.CreateCitizenPeds();
        }

        public override void OnStop()
        {
            Console.WriteLine("Stopped");
        }
    }
}
