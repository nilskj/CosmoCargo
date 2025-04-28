"use client";

import React, { useState } from "react";
import {
  Card,
  CardHeader,
  CardTitle,
  CardDescription,
  CardContent,
} from "@/components/ui/card";
import { Package, Search, Eye } from "lucide-react";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { useQuery } from "@tanstack/react-query";
import { getShipments, ShipmentsFilter } from "@/services/shipment-service";
import { ShipmentStatus } from "@/model/types";

const OngoingShipments = () => {
  const [search, setSearch] = useState("");
  const [status, setStatus] = useState<ShipmentStatus | "">("");

  const { data: shipments, refetch } = useQuery({
    queryKey: ["shipments", search, status],
    queryFn: () => {
      const filter: ShipmentsFilter = {};
      if (search) filter.search = search;
      if (status) filter.status = status as ShipmentStatus;
      return getShipments(filter);
    },
  });

  const handleSearch = (e: React.ChangeEvent<HTMLInputElement>) => {
    setSearch(e.target.value);
  };

  const handleStatusChange = (e: React.ChangeEvent<HTMLSelectElement>) => {
    setStatus(e.target.value as ShipmentStatus | "");
  };

  const getProgressPercentage = (status: ShipmentStatus): number => {
    switch (status) {
      case ShipmentStatus.InTransit:
        return 60;
      case ShipmentStatus.Delivered:
        return 100;
      case ShipmentStatus.Assigned:
        return 20;
      default:
        return 0;
    }
  };

  return (
    <div className="max-w-6xl mx-auto">
      <div className="mb-6">
        <h1 className="text-2xl font-medium mb-2">Pågående Leveranser</h1>
        <p className="text-space-text-secondary">
          Följ dina pågående leveranser i realtid
        </p>
      </div>

      <div className="flex flex-col md:flex-row justify-between gap-4 mb-6">
        <div className="relative w-full md:w-auto md:flex-1 max-w-sm">
          <Search className="absolute left-3 top-2.5 h-5 w-5 text-space-text-secondary" />
          <Input
            placeholder="Sök fraktID eller destination..."
            className="pl-10 space-input w-full"
            value={search}
            onChange={handleSearch}
          />
        </div>

        <div className="flex gap-2">
          <select 
            className="space-input"
            value={status}
            onChange={handleStatusChange}
          >
            <option value="">Alla statusar</option>
            <option value={ShipmentStatus.Assigned}>Tilldelad</option>
            <option value={ShipmentStatus.InTransit}>I transit</option>
            <option value={ShipmentStatus.Delivered}>Levererad</option>
          </select>

          <Button className="space-button" onClick={() => refetch()}>
            Uppdatera
          </Button>
        </div>
      </div>

      <div className="grid gap-6">
        {shipments?.map((shipment) => (
          <Card key={shipment.id} className="space-card overflow-hidden">
            <CardHeader className="pb-4">
              <div className="flex justify-between items-start">
                <div>
                  <CardTitle className="flex items-center text-xl">
                    <Package className="h-5 w-5 mr-2" />
                    {shipment.id}
                  </CardTitle>
                  <CardDescription>Skickad: {shipment.createdAt}</CardDescription>
                </div>
                <div className="text-right">
                  <div className="font-medium">{shipment.status}</div>
                  <div className="text-sm text-space-text-secondary">
                    Pilot: {shipment.pilotId || "Inte tilldelad"}
                  </div>
                </div>
              </div>
            </CardHeader>
            <CardContent>
              <div className="mb-6">
                <div className="flex justify-between mb-2 text-sm">
                  <div>{shipment.sender.planet}</div>
                  <div>{shipment.receiver.planet}</div>
                </div>
                <div className="relative pt-1">
                  <div className="overflow-hidden h-2 text-xs flex rounded bg-space-secondary/50">
                    <div
                      style={{ width: `${getProgressPercentage(shipment.status)}%` }}
                      className="shadow-none flex flex-col text-center whitespace-nowrap text-white justify-center bg-gradient-to-r from-space-accent-blue to-space-accent-purple"
                    ></div>
                  </div>
                </div>
              </div>

              <div className="flex justify-between items-center">
                <div className="text-space-text-secondary text-sm">
                  Last: {shipment.category}
                </div>
                <Button size="sm" variant="outline">
                  <Eye className="h-4 w-4 mr-2" />
                  Visa detaljer
                </Button>
              </div>
            </CardContent>
          </Card>
        ))}
      </div>
    </div>
  );
};

export default OngoingShipments;
