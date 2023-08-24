import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ActiveCompanyBankAccountComponent } from './active-company-bank-account.component';

describe('ActiveCompanyBankAccountComponent', () => {
  let component: ActiveCompanyBankAccountComponent;
  let fixture: ComponentFixture<ActiveCompanyBankAccountComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ActiveCompanyBankAccountComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ActiveCompanyBankAccountComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
