import { UserRole } from './types';

export default interface User {
    id: string;
    name: string;
    email: string;
    role: UserRole;
    createdAt: string;
    // Pilot-specific properties
    experience?: string;
    isActive?: boolean;
}
