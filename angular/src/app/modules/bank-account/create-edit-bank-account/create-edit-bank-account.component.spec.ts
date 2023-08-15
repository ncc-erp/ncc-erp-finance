import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { CreateEditBankAccountComponent } from './create-edit-bank-account.component';

describe('CreateEditBankAccountComponent', () => {
  let component: CreateEditBankAccountComponent;
  let fixture: ComponentFixture<CreateEditBankAccountComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ CreateEditBankAccountComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(CreateEditBankAccountComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
