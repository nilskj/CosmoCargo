import { ShipmentStatus, RiskLevel } from './types';

export default interface Shipment {
    id: string;
    customerId: string;
    pilotId?: string | null;
    origin: string;
    destination: string;
    weight: number;
    cargo: string;
    priority: string;
    status: ShipmentStatus;
    riskLevel: RiskLevel;
    scheduledDate?: string | null;
    createdAt: string;
    updatedAt: string;
}
