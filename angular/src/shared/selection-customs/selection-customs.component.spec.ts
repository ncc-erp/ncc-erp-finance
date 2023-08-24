import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { SelectionCustomsComponent } from './selection-customs.component';

describe('SelectionCustomsComponent', () => {
  let component: SelectionCustomsComponent;
  let fixture: ComponentFixture<SelectionCustomsComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ SelectionCustomsComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(SelectionCustomsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
