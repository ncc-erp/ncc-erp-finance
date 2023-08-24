import { Component, Inject, Injector, OnInit } from '@angular/core';
import { MAT_DIALOG_DATA } from '@angular/material/dialog';
import { AppComponentBase } from '@shared/app-component-base';

@Component({
  selector: 'app-create-edit-status',
  templateUrl: './create-edit-status.component.html',
  styleUrls: ['./create-edit-status.component.css']
})
export class CreateEditStatusComponent extends AppComponentBase implements OnInit {

  constructor(inject: Injector, @Inject(MAT_DIALOG_DATA) public data: any) {
    super(inject);
  }

  isEdit: boolean;
  ngOnInit(): void {
    this.isEdit = this.data.id === undefined ? false : true;
  }


}
