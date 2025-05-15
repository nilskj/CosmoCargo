import { ShipmentStatus } from './types';
import User from './user';

export interface ShipmentContact {
    name: string;
    email: string;
    planet: string;
    station: string;
}

export interface CustomsForm {
    containsLifeforms: boolean;
    lifeformType?: string | null;
    isPlasmaActive: boolean;
    plasmaStabilityLevel?: number | null;
    originPlanetLawsConfirmed: boolean;
    customsNotes?: string | null;
    quarantineRequired?: boolean | null;
}

export default interface Shipment {
    id: string;
    customerId: string;
    pilotId?: string | null;
    sender: ShipmentContact;
    receiver: ShipmentContact;
    weight: number;
    category: string;
    priority: string;
    description?: string | null;
    hasInsurance: boolean;
    status: ShipmentStatus;
    createdAt: string;
    updatedAt: string;
    pilot?: User | null;
    customs?: CustomsForm;
}
