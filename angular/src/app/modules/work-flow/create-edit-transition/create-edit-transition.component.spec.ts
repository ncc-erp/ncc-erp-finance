import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { CreateEditTransitionComponent } from './create-edit-transition.component';

describe('CreateEditTransitionComponent', () => {
  let component: CreateEditTransitionComponent;
  let fixture: ComponentFixture<CreateEditTransitionComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ CreateEditTransitionComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(CreateEditTransitionComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
