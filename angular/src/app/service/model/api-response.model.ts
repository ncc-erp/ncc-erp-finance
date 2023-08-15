import { PagedResultDto } from "@shared/paged-listing-component-base";

export class Response{
    success: boolean;
    error?: any;
    targetUrl?: string;
    unAuthorizedRequest: boolean;
}

export class ApiResponse<T> extends Response{
    result?: T;
}

export class ApiPagingResponse<T> extends Response {
    result?: {
        totalCount: number;
        items: T;
    };
}
