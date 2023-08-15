import { ListBTransactionLogComponent } from './list-b-transaction-log/list-b-transaction-log.component';
import { BTransactionLogComponent } from './b-transaction-log.component';
import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';


const routes: Routes = [
  {
    path: '', component: BTransactionLogComponent, children: [
      { path: '', pathMatch: 'full', component: ListBTransactionLogComponent },
      { path: 'list-btransaction-log', component: ListBTransactionLogComponent }
    ]
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class BTransactionLogRoutingModule { }
