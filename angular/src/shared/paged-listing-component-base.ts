import { AppComponentBase } from 'shared/app-component-base';
import { Component, Injector, OnInit } from '@angular/core';
import { ActivatedRoute, Params, Router } from '@angular/router';
import { AppConsts, OPTION_ALL } from './AppConsts';

export class PagedResultDto {
    items: any[];
    totalCount: number;
}
export class FilterDto {
    propertyName: string;
    value: any;
    comparision: number;
    filterType?: number
}
export class EntityDto {
    id: number;
}

export class PagedRequestDto {
    skipCount?: number;
    maxResultCount?: number;
    searchText?: string;
    filterItems?: FilterDto[] = [];
    sort: string;
    sortDirection: number;
}
export class PagedResultResultDto {
    result: PagedResultDto;
}
export interface tableHeaderDto {
    name: string,
    value: boolean,
    fieldName: string,
}

@Component({
    template: ''
})
export abstract class PagedListingComponentBase<TEntityDto> extends AppComponentBase implements OnInit {
    [x: string]: any;
    public pageSize: number = 5;
    public pageNumber: number = 1;
    public totalPages: number = 1;
    public totalItems: number;
    public searchText: string = '';
    public filterItems: FilterDto[] = [];
    public pageSizeType: number = 20;
    public advancedFiltersVisible: boolean = false;
    public emptyFilter: boolean = false;
    isTableLoading: boolean = false;
    activatedRoute: ActivatedRoute;
    router: Router;
    constructor(injector: Injector) {
        super(injector);
        this.activatedRoute = injector.get(ActivatedRoute);
        this.router = injector.get(Router);
        this.activatedRoute.queryParams.subscribe(params => {
            this.pageNumber = params['pageNumber'] ? params['pageNumber'] : 1;
            this.pageSize = params['pageSize'] ? params['pageSize'] : 20;
            this.searchText = params['searchText'] ? params['searchText'] : '';
            this.filterItems = params['filterItems'] ? JSON.parse(params['filterItems']) : [];
            this.advancedFiltersVisible = this.filterItems.length > 0;
            this.pageSizeType = Number(params['pageSize'] ? params['pageSize'] : 20);
        });

    }

    ngOnInit(): void {
        this.refresh();
    }
    checkAddFilter() {
        this.advancedFiltersVisible = !this.advancedFiltersVisible;
        if (this.filterItems.length === 0) {
            this.addFilter();
        }

    }
    refresh(): void {
        this.getDataPage(this.pageNumber);
    }

    getFirstPage() {
        this.getDataPage(AppConsts.FIRST_PAGE);
    }

    public showPaging(result: PagedResultDto, pageNumber: number): void {
        this.totalPages = ((result.totalCount - (result.totalCount % this.pageSize)) / this.pageSize) + 1;
        this.totalItems = result.totalCount;
        this.pageNumber = pageNumber;
    }

    public getDataPage(page: number): void {
        const req = new PagedRequestDto();
        req.maxResultCount = this.pageSize;
        req.skipCount = (page - 1) * this.pageSize;
        req.filterItems = this.filterItems;
        if (this.filterItems.length > 0) {
            req.filterItems.forEach((item, index) => {
                if (item.propertyName == "") {
                    req.filterItems.splice(index, 1)
                }
            })
        }
        this.advancedFiltersVisible = this.filterItems.length > 0;
        req.searchText = this.searchText;
        this.isTableLoading = true;
        this.pageNumber = page;
        this.router.navigate([], {
            queryParamsHandling: "merge",
            replaceUrl: true,
            queryParams: { pageNumber: this.pageNumber, pageSize: this.pageSize, searchText: this.searchText, filterItems: JSON.stringify(this.filterItems) }
        })
            .then(_ => this.list(req, page, () => {
                this.isTableLoading = false;
            }));
    }

    public deleteFilterItem(index: number) {
        this.filterItems.splice(index, 1);
        this.emptyFilter = true

    }
    public addFilter() {
        this.filterItems.push({
            propertyName: '',
            comparision: 0,
            value: '',
        });
    }
    public onEmitChange(event, i) {
        const { name, value } = event
        this.filterItems[i][name] = value
    }
    changePageSize() {
        // if (this.pageSize > this.totalItems) {
        //     this.pageNumber = 1;
        // }
        this.pageNumber = 1;
        this.pageSize = this.pageSizeType;
        this.refresh();
    }
    AddFilterItem(request: PagedRequestDto, propertyName: string, value: any) {
        let filterList = request.filterItems
        if (value !== "" || value == 0) {
            filterList.unshift({ propertyName: propertyName, comparision: 0, value: value })
        }
        return filterList
    }
    clearFilter(request: PagedRequestDto, propertyName: string, value: any) {
        let filterList = request.filterItems
        if (value !== "" || value == 0) {
            let item = filterList.filter(item => item.propertyName === propertyName)[0]
            filterList.splice(request.filterItems.indexOf(item), 1)
        }
        return filterList
    }
    setTableHeader(tableHeader: tableHeaderDto[], TABLE_NAME: string) {
        let currentColumnList: string = localStorage.getItem(TABLE_NAME);
        if (!currentColumnList) {
            localStorage.setItem(TABLE_NAME, JSON.stringify(tableHeader));
        }
        else {
            if (this.compareWithLocalStorage(currentColumnList, tableHeader)) {
                tableHeader = JSON.parse(currentColumnList);
            }
            else {
                localStorage.setItem(TABLE_NAME, JSON.stringify(tableHeader));
            }
        }
        return tableHeader;
    }

    compareWithLocalStorage(listToCompare: string, tableHeader: tableHeaderDto[]) {
        //listToCompare = listToCompare.map(item => item.fieldName);
        let listFielNameToCompare = JSON.parse(listToCompare).map(item => item.fieldName);
        let currentColumnList = tableHeader.map(item => item.fieldName);
        if (JSON.stringify(listFielNameToCompare) === JSON.stringify(currentColumnList)) {
            return true;
        }
        return false;
    }

    public onDropdownFilter(value: any, propertyName?: string) {
        this.clearDuplicateFilter(propertyName)
        if (value != -1) {
            let filterItem = { propertyName: propertyName, value: value, comparision: 0 } as FilterDto
            this.filterItems.push(filterItem)
        }
        this.getDataPage(1)
    }

    private clearDuplicateFilter(propertyName: string) {
        this.filterItems.forEach(item => {
            if (item.propertyName == propertyName) {
                this.filterItems.splice(this.filterItems.indexOf(item), 1)
            }
        })
    }

    public setFilterToUrl(filterName:string, value:any){
        let queryParams = {}
        if( value === 0 || (value && value != OPTION_ALL)){
            queryParams[filterName] = typeof(value) != "string" ? JSON.stringify(value) : value
        }
        else{
            queryParams[filterName] = null
        }

        this.router.navigate([], {
            queryParamsHandling: "merge",
            replaceUrl: true,
            queryParams: queryParams
        })
    }

    public onPageFilter(filterName:string, value:any){
        this.getDataPage(1)
        this.setFilterToUrl(filterName, value)
    }

    public async resetQueryParams(listParam:string[]){
        let queryParams: Params = {}

        listParam.forEach(param => {
            queryParams[param] = null
        })

        queryParams['searchText'] = this.searchText
        await this.router.navigate(
          [],
          {
            queryParams: queryParams,
            queryParamsHandling: 'merge',
          });
      }

    protected abstract list(request: PagedRequestDto, pageNumber: number, finishedCallback: Function): void;
    protected abstract delete(entity: TEntityDto): void;
}
