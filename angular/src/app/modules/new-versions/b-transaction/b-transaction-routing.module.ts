import { ListBtransactionComponent } from './list-btransaction/list-btransaction.component';
import { BTransactionComponent } from './b-transaction.component';
import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';


const routes: Routes = [
  {
    path: '', component: BTransactionComponent, children: [
      { path: '', pathMatch: 'full', component: ListBtransactionComponent },
      { path: 'list-btransaction', component: ListBtransactionComponent }
    ]
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class BTransactionRoutingModule { }
