import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { TreeInOutTypeComponent } from './tree-in-out-type.component';

describe('TreeInOutTypeComponent', () => {
  let component: TreeInOutTypeComponent;
  let fixture: ComponentFixture<TreeInOutTypeComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ TreeInOutTypeComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(TreeInOutTypeComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
