import { RevenueExpenseType } from "@shared/AppEnums"

export interface CircleChartDto {
    id: number
    name: string
    isIncome: boolean
    isActive: boolean
    circleChartTypeName: string
}

export interface CreateCircleChartDto{
    name: string
    isIncome: boolean
}

export interface CircleChartInfoDto extends CircleChartDto {
    details: CircleChartDetailInfoDto[]
    allClientIds: number[]
    allInOutcomeTypeIds: number[]
}

export interface CircleChartDetailInfoDto {
    id: number
    circleChartId: number
    name: string
    color: string
    revenueExpenseType: number
    branchId: number
    branch: BranchInfoDto
    clients: ClientInfoDto[]
    inOutcomeTypes: InOutcomeTypeDto[]
    listClientIds: number[]
    listInOutcomeTypeIds: number[]
    hideClient : boolean;
    hideEntryType: boolean;
}

export interface ClientInfoDto {
    clientId: number
    clientName: string
}

export interface InOutcomeTypeDto {
    inOutcomeTypeId: number
    inOutcomeTypeName: string
}

export interface BranchInfoDto {
    branchId: number
    branchName: string
}

export class CreateCircleChartDetailDto
{
    circleChartId: number
    name: string
    color: string
    branchId: number
    revenueExpenseType: number
}

export class UpdateCircleChartDetailDto
{
    id: number
    name: string
    color: string
    branchId: number
    clientIds: number[]
    revenueExpenseType: number
}

export class UpdateCircleChartInOutcomeTypeIdsDto
{
    id: number
    inOutcomeTypeIds: number[]
}

export class InputListCircleChartDto
{
    circleChartIds: number[]
    startDate: string
    endDate: string
}

export class ResultCircleChartDto
{
    id: number;
    chartName: string;
    isIncome: boolean;
    details: ResultCircleChartDetailDto[];
}
export class ResultCircleChartDetailDto
{
    id: number;
    circleChartId: number;
    name: string;
    value: number;
    color: string;
}