import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { EditTransitionComponent } from './edit-transition.component';

describe('EditTransitionComponent', () => {
  let component: EditTransitionComponent;
  let fixture: ComponentFixture<EditTransitionComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ EditTransitionComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(EditTransitionComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
