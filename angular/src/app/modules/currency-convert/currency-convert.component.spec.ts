import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { CurrencyConvertComponent } from './currency-convert.component';

describe('CurrencyConvertComponent', () => {
  let component: CurrencyConvertComponent;
  let fixture: ComponentFixture<CurrencyConvertComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ CurrencyConvertComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(CurrencyConvertComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
