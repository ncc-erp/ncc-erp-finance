import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { CreateEditCurrencyConvertComponent } from './create-edit-currency-convert.component';

describe('CreateEditCurrencyConvertComponent', () => {
  let component: CreateEditCurrencyConvertComponent;
  let fixture: ComponentFixture<CreateEditCurrencyConvertComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ CreateEditCurrencyConvertComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(CreateEditCurrencyConvertComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
