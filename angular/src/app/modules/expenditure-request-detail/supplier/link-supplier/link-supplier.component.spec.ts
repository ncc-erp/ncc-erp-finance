import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { LinkSupplierComponent } from './link-supplier.component';

describe('LinkSupplierComponent', () => {
  let component: LinkSupplierComponent;
  let fixture: ComponentFixture<LinkSupplierComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ LinkSupplierComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(LinkSupplierComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
