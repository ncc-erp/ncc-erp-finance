import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { CreateEditRequestComponent } from './create-edit-request.component';

describe('CreateEditRequestComponent', () => {
  let component: CreateEditRequestComponent;
  let fixture: ComponentFixture<CreateEditRequestComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ CreateEditRequestComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(CreateEditRequestComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
