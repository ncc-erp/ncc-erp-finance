import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { CreateEditSupplierComponent } from './create-edit-supplier.component';

describe('CreateEditSupplierComponent', () => {
  let component: CreateEditSupplierComponent;
  let fixture: ComponentFixture<CreateEditSupplierComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ CreateEditSupplierComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(CreateEditSupplierComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
