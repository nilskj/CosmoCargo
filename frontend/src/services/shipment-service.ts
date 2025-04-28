import { api } from "./api";
import Shipment, { ShipmentContact } from "../model/shipment";
import { ShipmentStatus } from "../model/types";

export interface CreateShipmentRequest {
    origin: ShipmentContact;
    destination: ShipmentContact;
    weight: number;
    category: string;
    priority: string;
    description?: string | null;
    hasInsurance: boolean;
}

export interface UpdateShipmentStatusRequest {
    status: ShipmentStatus;
}

export interface AssignPilotRequest {
    pilotId: string;
}

export interface ShipmentsFilter {
    search?: string;
    status?: ShipmentStatus;
}

export const getShipments = async (filter?: ShipmentsFilter): Promise<Shipment[]> => {
    const queryParams = new URLSearchParams();
    if (filter?.search) queryParams.append('search', filter.search);
    if (filter?.status) queryParams.append('status', filter.status);
    
    const response = await api.get<Shipment[]>(`/shipments?${queryParams.toString()}`);
    return response.data;
};

export const getShipmentById = async (id: string): Promise<Shipment> => {
    const response = await api.get<Shipment>(`/shipments/${id}`);
    return response.data;
};

export const createShipment = async (request: CreateShipmentRequest): Promise<Shipment> => {
    const response = await api.post<Shipment>('/shipments', request);
    return response.data;
};

export const updateShipmentStatus = async (id: string, request: UpdateShipmentStatusRequest): Promise<Shipment> => {
    const response = await api.put<Shipment>(`/shipments/${id}/status`, request);
    return response.data;
};

export const assignPilot = async (id: string, request: AssignPilotRequest): Promise<Shipment> => {
    const response = await api.put<Shipment>(`/shipments/${id}/assign-pilot`, request);
    return response.data;
};

