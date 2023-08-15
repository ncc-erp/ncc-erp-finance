import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { CustomeSelectComponent } from './custome-select.component';

describe('CustomeSelectComponent', () => {
  let component: CustomeSelectComponent;
  let fixture: ComponentFixture<CustomeSelectComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ CustomeSelectComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(CustomeSelectComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
