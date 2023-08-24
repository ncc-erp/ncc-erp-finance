import { Injectable } from '@angular/core';
import {
    ActivatedRouteSnapshot, Resolve,
    RouterStateSnapshot
} from '@angular/router';
import {UtilitiesService} from '../../../service/api/new-versions/utilities.service';

@Injectable({
    providedIn: 'root'
})
export class NRevenueResolver implements Resolve<void> {
    public accounts = [];
    constructor(
        public _utilities: UtilitiesService,

    ) { }

    resolve(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): Promise<void> {
        return this._utilities.loadCatalogForRevenue();
    }

}
