export interface Role {
  roleId: string;
  name: string;
}

export interface UserRole {
  roleId: string;
  userId: string;
  roleName: string;
}

export interface UserRoleRequest {
  userId: string;
  roleId: string;
}