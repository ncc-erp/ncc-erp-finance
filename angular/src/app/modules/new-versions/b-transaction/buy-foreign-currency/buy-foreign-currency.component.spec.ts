import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { BuyForeignCurrencyComponent } from './buy-foreign-currency.component';

describe('BuyForeignCurrencyComponent', () => {
  let component: BuyForeignCurrencyComponent;
  let fixture: ComponentFixture<BuyForeignCurrencyComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ BuyForeignCurrencyComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(BuyForeignCurrencyComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
