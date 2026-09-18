export interface AlertSource {
  id: string;
  title: string;
  externalUrl: string;
  country: string;
  createdAtUtc: string;
  updatedAtUtc?: string | null;
}

export interface AlertSourceCreateRequest {
  title: string;
  externalUrl: string;
  country: string;
}

export interface AlertSourceUpdateRequest {
  title: string;
  externalUrl: string;
  country: string;
}
