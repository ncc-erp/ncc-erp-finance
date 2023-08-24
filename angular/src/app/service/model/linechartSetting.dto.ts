export interface LineChartSettingDto {
    id:number
    name: string
    type: number
    isActive: boolean
    color: string
    listReference: ReferenceInfo[]
    lineChartSettingTypeName:string
}

export interface AddLinechartSettingDto{
    linechartId:number
    referenceId:number
}

export interface ReferenceInfo{
     id :number
     name :string
}
