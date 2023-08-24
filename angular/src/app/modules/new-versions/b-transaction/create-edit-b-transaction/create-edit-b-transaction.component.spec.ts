import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { CreateEditBTransactionComponent } from './create-edit-b-transaction.component';

describe('CreateEditBTransactionComponent', () => {
  let component: CreateEditBTransactionComponent;
  let fixture: ComponentFixture<CreateEditBTransactionComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ CreateEditBTransactionComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(CreateEditBTransactionComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
