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
    branch: BranchInfoDto
    clients: ClientInfoDto[]
    inOutcomeTypes: InOutcomeTypeDto[]
    listClientIds: number[]
    listInOutcomeTypeIds: number[]
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
}

export class UpdateCircleChartDetailDto
{
    id: number
    name: string
    color: string
    branchId: number
    clientIds: number[]
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
