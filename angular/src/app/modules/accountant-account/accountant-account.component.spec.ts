import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { AccountantAccountComponent } from './accountant-account.component';

describe('AccountantAccountComponent', () => {
  let component: AccountantAccountComponent;
  let fixture: ComponentFixture<AccountantAccountComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ AccountantAccountComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(AccountantAccountComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
