import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { CreateEditDialogStatusComponent } from './create-edit-dialog-status.component';

describe('CreateEditDialogStatusComponent', () => {
  let component: CreateEditDialogStatusComponent;
  let fixture: ComponentFixture<CreateEditDialogStatusComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ CreateEditDialogStatusComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(CreateEditDialogStatusComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
