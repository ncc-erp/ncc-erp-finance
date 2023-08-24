import { ListNRevenueComponent } from './list-n-revenue/list-n-revenue.component';
import { NRevenueComponent } from './n-revenue.component';
import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import {NRevenueResolver} from './n-revenue.resolver';


const routes: Routes = [
  {
    path: '', component: NRevenueComponent, children: [
      { path: '', pathMatch: 'full', component: ListNRevenueComponent, resolve: {revenueResolver: NRevenueResolver} },
      { path: 'list-nrevenue', component: ListNRevenueComponent, resolve: {revenueResolver: NRevenueResolver} }
    ]
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class NRevenueRoutingModule { }
