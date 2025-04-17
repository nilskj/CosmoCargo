"use client";

import React from "react";
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

// Sample data
const shipments = [
  {
    id: "SC-1234567",
    from: "Jorden, Alpha Station",
    to: "Mars, Olympus Station",
    date: "2023-04-10",
    status: "I transit",
    progress: 60,
    pilot: "Anna Karlsson",
    ship: "Stellar Phoenix IX",
  },
  {
    id: "SC-7654321",
    from: "Jorden, Beta Station",
    to: "Europa, Ice Harbor",
    date: "2023-04-12",
    status: "Förbereder",
    progress: 20,
    pilot: "Marcus Lindqvist",
    ship: "Quantum Voyager",
  },
  {
    id: "SC-9876543",
    from: "Mars, Olympus Station",
    to: "Jorden, Gamma Station",
    date: "2023-04-08",
    status: "Närmar sig destination",
    progress: 85,
    pilot: "Elsa Berg",
    ship: "Nebula Sprinter",
  },
];

const OngoingShipments = () => {
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
          />
        </div>

        <div className="flex gap-2">
          <select className="space-input">
            <option value="">Alla statusar</option>
            <option value="preparing">Förbereder</option>
            <option value="in-transit">I transit</option>
            <option value="approaching">Närmar sig destination</option>
          </select>

          <Button className="space-button">Uppdatera</Button>
        </div>
      </div>

      <div className="grid gap-6">
        {shipments.map((shipment) => (
          <Card key={shipment.id} className="space-card overflow-hidden">
            <CardHeader className="pb-4">
              <div className="flex justify-between items-start">
                <div>
                  <CardTitle className="flex items-center text-xl">
                    <Package className="h-5 w-5 mr-2" />
                    {shipment.id}
                  </CardTitle>
                  <CardDescription>Skickad: {shipment.date}</CardDescription>
                </div>
                <div className="text-right">
                  <div className="font-medium">{shipment.status}</div>
                  <div className="text-sm text-space-text-secondary">
                    Pilot: {shipment.pilot}
                  </div>
                </div>
              </div>
            </CardHeader>
            <CardContent>
              <div className="mb-6">
                <div className="flex justify-between mb-2 text-sm">
                  <div>{shipment.from}</div>
                  <div>{shipment.to}</div>
                </div>
                <div className="relative pt-1">
                  <div className="overflow-hidden h-2 text-xs flex rounded bg-space-secondary/50">
                    <div
                      style={{ width: `${shipment.progress}%` }}
                      className="shadow-none flex flex-col text-center whitespace-nowrap text-white justify-center bg-gradient-to-r from-space-accent-blue to-space-accent-purple"
                    ></div>
                  </div>
                </div>
              </div>

              <div className="flex justify-between items-center">
                <div className="text-space-text-secondary text-sm">
                  Skepp: {shipment.ship}
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
